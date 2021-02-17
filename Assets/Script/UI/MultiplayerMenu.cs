using Assets.Script.Client;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
    public GameObject privateButton, findMatchButton, tCPClient,
            privateCreateButton, nameRoomField, withPasswordCheckBox, passwordRoomField, createPrivateRoom,
            privateFindButton, findPrivateRoom, error, sceneController;

    public void DoWind()
    {
        if (!sceneController.GetComponent<GameController>().EditingTable())
        {
            findPrivateRoom.SetActive(false);
            createPrivateRoom.SetActive(false);
            nameRoomField.SetActive(false);
            passwordRoomField.SetActive(false);
            withPasswordCheckBox.SetActive(false);
            privateFindButton.SetActive(false);
            privateCreateButton.SetActive(false);
            error.SetActive(false);

            privateButton.GetComponent<Button>().interactable = true;
            findMatchButton.GetComponent<Button>().interactable = true;
            findPrivateRoom.GetComponent<Button>().interactable = true;
            createPrivateRoom.GetComponent<Button>().interactable = true;
            nameRoomField.GetComponent<InputField>().interactable = true;
            passwordRoomField.GetComponent<InputField>().interactable = true;
            withPasswordCheckBox.GetComponent<Toggle>().interactable = true;

            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2( -530 , -2710);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(165, -1255);            

            privateButton.SetActive(true);
            findMatchButton.SetActive(true);
            gameObject.SetActive(true);
        }
    } 
    public void SelectedOnlineMode(bool isPrivate)
    {
        if (isPrivate)
        {
            privateButton.SetActive(false);
            findMatchButton.SetActive(false);

            privateFindButton.SetActive(true);
            privateCreateButton.SetActive(true);
        }
        else
        {
            privateButton.GetComponent<Button>().interactable = false;
            findMatchButton.GetComponent<Button>().interactable = false;
        }
    }

    public void SelectedPrivate(bool create)
    {
        privateFindButton.SetActive(false);
        privateCreateButton.SetActive(false);

        nameRoomField.SetActive(true);
        withPasswordCheckBox.SetActive(true);
        if (!create)
            findPrivateRoom.SetActive(true);
        else
            createPrivateRoom.SetActive(true);
    }

    public void ChangedNeedsPassword(bool isOn)
    {
        int offset = 140;
        if (!isOn) 
        {
            passwordRoomField.GetComponent<InputField>().text = "";
            offset = -140;
        }

        float y = gameObject.GetComponent<RectTransform>().sizeDelta.y;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, y + offset);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(gameObject.GetComponent<RectTransform>().anchoredPosition.x,
           gameObject.GetComponent<RectTransform>().anchoredPosition.y + offset / 2);

        Vector2 pos = nameRoomField.GetComponent<RectTransform>().anchoredPosition;
        nameRoomField.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y + offset);

        passwordRoomField.SetActive(isOn);
    }

    public void Room(bool create)
    {
        findPrivateRoom.GetComponent<Button>().interactable = false;
        createPrivateRoom.GetComponent<Button>().interactable = false;

        nameRoomField.GetComponent<InputField>().interactable = false;
        passwordRoomField.GetComponent<InputField>().interactable = false;

        withPasswordCheckBox.GetComponent<Toggle>().interactable = false;

        if (create) Send.CreateRoom(nameRoomField.GetComponent<InputField>().text,
                                passwordRoomField.GetComponent<InputField>().text);
        else        Send.FindPrivateRoom(nameRoomField.GetComponent<InputField>().text,
                                     passwordRoomField.GetComponent<InputField>().text);
    }

    internal void ConnectToRoom()
    {
        gameObject.SetActive(false);
    }

    internal void ErrorFindPrivateRoom(string message)
    {
        error.GetComponent<Text>().text = message;
        error.SetActive(true);
        float y = gameObject.GetComponent<RectTransform>().sizeDelta.y;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, y + 100);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(gameObject.GetComponent<RectTransform>().anchoredPosition.x,
           gameObject.GetComponent<RectTransform>().anchoredPosition.y + 50);
    }
    public void Ychange()
    {
        Debug.Log(gameObject.GetComponent<RectTransform>().anchoredPosition.x + " " + gameObject.GetComponent<RectTransform>().anchoredPosition.y);
    }
}
