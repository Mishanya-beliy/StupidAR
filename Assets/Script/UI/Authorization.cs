using Assets.Script.Client;
using UnityEngine;
using UnityEngine.UI;

public class Authorization : MonoBehaviour
{
    public GameObject loginField, passwordField, registrationButton, authorizationButton, stayAuthorizatedCheckBox,
        multiplayerMenu, process, answerMessage, tCPClient;
    
    public void DoWind()
    {
        process.SetActive(false);
        answerMessage.SetActive(false);
        loginField.GetComponent<InputField>().interactable = true;
        passwordField.GetComponent<InputField>().interactable = true;

        gameObject.SetActive(true);
        if (tCPClient.GetComponent<Client>().authorizated)
            HideAuthorizationShowMultiplayerMenu();
        else if (Registration())
        {
            loginField.GetComponent<InputField>().text = registeredLogin;
            passwordField.GetComponent<InputField>().text = registeredPassword;
            if (stayAuthorizatedCheckBox.GetComponent<Toggle>().isOn)
            {
                RegAndAuthGeneralDoList(false, authorizationButton, authorizatingProcess, registeredLogin, registeredPassword);
            }
            else ReplaceRegistrationAndAuthorization(true);
        }
        else ReplaceRegistrationAndAuthorization(false);
    }

    private void ReplaceRegistrationAndAuthorization(bool registration)
    {
        authorizationButton.SetActive(registration);
        registrationButton.SetActive(!registration);
    }

    private bool isRegistered;
    private bool Registration()
    {
        return isRegistered;
    }

    private const string registeringProcess = "Registering...";
    public void Registering()
    {
        answerMessage.SetActive(false);

        RegAndAuthGeneralDoList(true, registrationButton, registeringProcess,
        loginField.GetComponent<InputField>().text, passwordField.GetComponent<InputField>().text);
    }

    private const string successfulRegistration = "Registration is successful.";
    public string registeredLogin { get; private set; } = "beliy";
    public static string registeredPassword { get; private set; }  = "1488";
    public void Registered(string answer)
    {
        loginField.GetComponent<InputField>().interactable = true;
        passwordField.GetComponent<InputField>().interactable = true;
        process.SetActive(false);

        if (answer == successfulRegistration)
        {
            registeredLogin = loginField.GetComponent<InputField>().text;
            registeredPassword = passwordField.GetComponent<InputField>().text;

            isRegistered = true;

            if (stayAuthorizatedCheckBox.GetComponent<Toggle>().isOn)
                RegAndAuthGeneralDoList(false, authorizationButton, authorizatingProcess, 
                    loginField.GetComponent<InputField>().text, passwordField.GetComponent<InputField>().text);
            else
                authorizationButton.SetActive(true);
        }
        else
        {
            answerMessage.SetActive(true);
            answerMessage.GetComponent<Text>().text = answer;

            registrationButton.SetActive(true);
        }
    }

    private const string authorizatingProcess = "Authorizating...";
    public void Authorizating()
    {
        answerMessage.SetActive(false);

        RegAndAuthGeneralDoList(false, authorizationButton, authorizatingProcess,
        loginField.GetComponent<InputField>().text, passwordField.GetComponent<InputField>().text);
    }


    private const string successfulAuthorization = "Authorization is successful.";
    public void Authorized(string answer)
    {
        loginField.GetComponent<InputField>().interactable = true;
        passwordField.GetComponent<InputField>().interactable = true;
        process.SetActive(false);

        if (answer == successfulAuthorization)
        {
            Client.login = loginField.GetComponent<InputField>().text;
            Client.password = passwordField.GetComponent<InputField>().text; ;

            HideAuthorizationShowMultiplayerMenu();
        }
        else
        {
            answerMessage.SetActive(true);
            answerMessage.GetComponent<Text>().text = answer;

            authorizationButton.SetActive(true);
        }
    }
    private void RegAndAuthGeneralDoList(bool reg, GameObject button, string proccesText, string login, string password)
    {
        button.SetActive(false);

        loginField.GetComponent<InputField>().interactable = false;
        passwordField.GetComponent<InputField>().interactable = false;

        Send.RegistrationOrAuthorization(!reg, login, password);

        process.GetComponent<Text>().text = proccesText;
        process.SetActive(true);
    }

    private void HideAuthorizationShowMultiplayerMenu()
    {
        gameObject.SetActive(false);
        multiplayerMenu.GetComponent<MultiplayerMenu>().DoWind();
    }
}
