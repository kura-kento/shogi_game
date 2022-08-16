using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;
using Photon.Pun;
using UnityEngine.EventSystems;
using TMPro;
//先攻プレーヤー
public class Player : PlayerController
{
    private GameObject parent;
    public static Player instance;
    // PhotonView photonView;
    string KOMADAI;
    bool isFirstPlayer;
    int[] player_1 = new int[] { 1, 2, 3, 4, 5 };
    int[] player_2 = new int[] { -6, -4, -8, -7, -1 };

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 先手後手の情報取得
        Debug.Log(PhotonMaster.GM.GetPlayerType());
        Debug.Log("PhotonNetwork.LocalPlayer:" + PhotonNetwork.LocalPlayer.ToString());
        Debug.Log(PhotonNetwork.LocalPlayer.ToString().Contains("#01"));

        //　ルーム作成者が先手　かつ　ルーム作成者の場合 または　ルーム作成者が後手　かつ　ルーム参加者の場合 
        isFirstPlayer = PhotonMaster.GM.GetPlayerType() == PLAYER_TYPE.FIRST && PhotonNetwork.LocalPlayer.ToString().Contains("#01") || PhotonMaster.GM.GetPlayerType() == PLAYER_TYPE.SECOND && PhotonNetwork.LocalPlayer.ToString().Contains("#02");
        KOMADAI = isFirstPlayer ? "Komadai1" : "Komadai2";

        SetupPhoton();
        // SetupKomadai();
        // GameMaster.MasuStatusLog();
    }

    // [PunRPC]
    public void SetupPhoton() {
        for(int i=0;i<5;i++) {
            var obj = PhotonNetwork.Instantiate("Koma", new Vector3(0, 0, 0), Quaternion.identity, 0);
            obj.AddComponent<Koma>();
            Koma koma = obj.transform.gameObject.GetComponent<Koma>();
            koma.number = isFirstPlayer ? player_1[i] : player_2[i];
            Transform children = obj.GetComponentInChildren<Transform>();
            koma.text = obj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();;
            KomaGenerator.instance.TextChange(koma);
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => { 
                    SelectKoma(koma);
                });
            trigger.triggers.Add(entry);

            koma.transform.parent = GameObject.Find(KOMADAI).transform;
            koma.tag = "Komadai";

            koma.transform.localPosition = new Vector3((App.MASU_SIZE * 2.0f) - (App.MASU_SIZE * i), 0, 0);
            koma.transform.localScale = new Vector3(App.MASU_SIZE, App.MASU_SIZE, App.MASU_SIZE);
            if(!isFirstPlayer) { koma.transform.Rotate(0, 0, 180.0f); };
        }
    }

//     void Update()
//     {
//         if (Input.GetMouseButtonDown(0)) {
//             Debug.Log("クリックしたよ");
//             photonView.RPC(nameof(RpcSendMessage), RpcTarget.All, "こんにちは");
//         }
//     }

//    [PunRPC]
//     private void RpcSendMessage(string message) {
//         Debug.Log(message);
//     }

    //初期配置
    public void SetupKomadai()
    {
        // GameObject komadai = GameObject.Find("Komadai1");
        //自身の駒
        for(int i=0;i<5;i++) {
            Koma Fu = KomaGenerator.instance.Spawn(player_1[i]);
            Fu.transform.parent = GameObject.Find("Komadai1").transform;
            Fu.tag = "Komadai";

            Fu.transform.localPosition = new Vector3(App.MASU_SIZE - 2.5f * i, 0, 0);
            Fu.ClickAction = SelectKoma; //クリックされた時関数を呼ぶ
        }
    }

    //コマクリック時(自分が生成した)
    public void SelectKoma(Koma koma)
    {   
        GameMaster.ResetMasu(); //リセット

        //同じ駒を押した時、キャンセルする。
        if(App.slot == koma) {
            App.slot = null;
            return;
        }

        GameObject parent = koma.transform.parent.gameObject;//駒の親要素を取得
        App.slot = koma;
        
        //「KOMA_SET」の時
        if(App.game_type == GAME_TYPE.KOMA_SET) {
            //全ての
            mySelectObj(isFirstPlayer: isFirstPlayer);
            return;
        }
        //駒台からの移動
        if (parent.name == KOMADAI) {
            Debug.Log("駒台からの移動");
            //選択できるマスを表示する(0のステータスのマスを色を変化させる)
            CreateSelectObj(isFirstPlayer: isFirstPlayer);
        }
        //盤上からの移動
        else if(parent.name == "Masu") {
            Debug.Log("盤上からの移動");
            SelectObj(koma, isFirstPlayer: isFirstPlayer);
        }
    }

    public Koma CreateKoma(int status) {
        Koma k = KomaGenerator.instance.Spawn(status);
        k.transform.parent = GameObject.Find(KOMADAI).transform;
        k.tag = "Komadai";
        k.ClickAction = SelectKoma; //クリックされた時関数を呼ぶ
        return k;
    }
}
// //初期動作
// //4*4 で行う外周を99で配置する enam positionning,+自分で追加
// //initPositioning _ids = [0,1,2,2,2];
// //onclick

// if ($isdai || $isBan) {
//     if($this->App.slot && false); // 置く
        
//     if(!$this->App.slot && true);  //取る
//     //その場所に対してのデータ  $this->App.slotに入れる
// }
// //5つになったら活性化させる

//裏にしたまま
// //範囲指定
// $item->require_flg;
// //範囲外の場合処理を中断する
// //DBの値を
// if ($item->id === 2) {
//     for($i=1; $i<10; $i++) {
//         //現在の値から - $i +$iする
//         if(true){ break;} //範囲判定(99 or 1) //共通化？
//         //要素追加
//     }
//    // -1 +1を確認して範囲ないなら要素を生成
// }
//どの『item』がおこなっている処理？
//選択する処理は共通化
//⓵『選択肢』を選んでその制御によった選択肢を表示する(範囲 0,-1 選んだitem_idによって変わる)
//⓶以外の『選択肢』を選ぶ(範囲 0のみ全て)
//共通処理 -1の時だけ別処理
//終了判定
//次の判定


// //判定ルール
// item_idが判定時に存在しない
// item_idが-4の位置に存在する。
// item_id==1以外がなくなる
// 処理時間が

// Player
// Koma App.slot = null;
// Awake()
// List<int> komas = List<int>(); 台に
// int komas[5] = [1,2,2,3,4]; 固定
// for(i=0; i<5; i++;)



//         if (Input.GetMouseButtonDown(0)) {
//             Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);
// //Physics2D.Raycast(原点、方向、長さ、指定レイヤー)
//             RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
 
//             if (hit2d) {
                
//                 clickedGameObject = hit2d.transform.gameObject;
//                 int x = (int)clickedGameObject.transform.position.x;
//                 int y = (int)clickedGameObject.transform.position.y;
//                 Debug.Log(clickedGameObject.name+"x" + x.ToString() + "y:" + y.ToString());

//                 // Debug.Log(clickedGameObject.name);
//                 if(clickedGameObject.name == "Koma(Clone)") {
//                     Debug.Log("成功");
//                 }
//             } 
//             Debug.Log(clickedGameObject);
//         }

            // Debug.Log(clickedGameObject);
            //  Debug.Log(clickedGameObject.transform.position);
            // clickedGameObject = null;
 
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
 
            // if (hit2d) {
            //     clickedGameObject = hit2d.transform.gameObject;
            // } 
 
            // Debug.Log(hit2d.transform.gameObject.position);