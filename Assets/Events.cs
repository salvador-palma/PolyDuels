using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Events : MonoBehaviour
{
    public void startPlayer(){
        PlayerNetwork.LocalInstance.current_speed = PlayerNetwork.speed;
        GameState.Instance.changeStateServerRpc();
        try{
            MiniGame.Instance.Init();
        }catch{
            Debug.Log("Usual Error");
        }
        
    }

    public void startGame(){
        
        CharacterSelect.Instance.CharSelectFadeOutEvent();
        
        
    }

    public void nextGame(){
        GameUI.Instance.next();
    }
    
    
}
