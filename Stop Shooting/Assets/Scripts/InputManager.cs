using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool choosing;
    private Dictionary<string, KeyCode> keyBindings;
    private string pendingKeyChange;
    private bool isDefault;
    private List<string> moveKeys = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        keyBindings = new Dictionary<string, KeyCode>();
        keyBindings.Add("shoot",KeyCode.Space);
        keyBindings.Add("pause", KeyCode.P);
        keyBindings.Add("special", KeyCode.E);
        keyBindings.Add("left", KeyCode.A);
        keyBindings.Add("right", KeyCode.D);
        keyBindings.Add("up", KeyCode.W);
        keyBindings.Add("down", KeyCode.S);
        //movement keys list to loop around in certain functions
        moveKeys.Add("left");
        moveKeys.Add("right");
        moveKeys.Add("up");
        moveKeys.Add("down");
        isDefault = true;
        //default keybinds

    }
    
    public KeyCode GetKeyBind(string name)
    {
        return keyBindings[name];
    }
    public void ChangeKey(string name)
    {
        pendingKeyChange = name;
        choosing = true;
        foreach (string key in moveKeys)
        {
            if (name == key)
            {
                isDefault = false;
                Debug.Log("Not default keys");
            }
        }
    }
    private void Update()
    {
        if (choosing && Input.anyKey)
        {
            foreach (KeyCode registeredKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(registeredKey))
                {
                    keyBindings[pendingKeyChange] = registeredKey;
                    Debug.Log(keyBindings[pendingKeyChange].ToString());
                    GetComponent<UIManager>().RefreshKeyBinds();
                    choosing = false;
                    break;
                }
            }
        }
    }
    public bool GetDefault()
    {
        return isDefault;
    }
}
