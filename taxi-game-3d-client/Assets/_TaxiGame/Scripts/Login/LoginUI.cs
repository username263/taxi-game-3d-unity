using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class LoginUI : MonoBehaviour
    {
        [SerializeField]
        GameObject loginContainer;
        [SerializeField]
        TMP_InputField emailField;
        [SerializeField]
        TMP_InputField passwordField;
        [SerializeField]
        Button loginButton;
        [SerializeField]
        Button registerButton;

        public static LoginUI Instance
        {
            get;
            private set;
        }

        void Awake()
        {
            Instance = this;
            SetLoginContainerVisible(false);
        }

        void Start()
        {
            emailField.onValueChanged.AddListener(text => UpdateLoginButtonInteractable());
            passwordField.onValueChanged.AddListener(text => UpdateLoginButtonInteractable());
            loginButton.onClick.AddListener(async () =>
            {
                SetLoginInteractable(false);
                var statusCode = await LoginLogic.Instance.Login(
                    emailField.text,
                    passwordField.text
                );
                if (statusCode.IsSuccess())
                {
                    LoginLogic.Instance.GotoGame();
                    return;
                }
                SetLoginInteractable(true);
            });
            registerButton.onClick.AddListener(async () =>
            {
                SetLoginInteractable(false);
                var statusCode = await LoginLogic.Instance.Register(
                    emailField.text,
                    passwordField.text
                );
                if (statusCode.IsSuccess())
                {
                    LoginLogic.Instance.GotoGame();
                    return;
                }
                SetLoginInteractable(true);
            });
        }

        public void SetLoginContainerVisible(bool visible)
        {
            loginContainer.SetActive(visible);
            UpdateLoginButtonInteractable();
        }

        void UpdateLoginButtonInteractable()
        {
            var interactable =
                    !string.IsNullOrEmpty(emailField.text) &&
                    !string.IsNullOrEmpty(passwordField.text);
            loginButton.interactable = interactable;
            registerButton.interactable = interactable;
        }

        void SetLoginInteractable(bool interactable)
        {
            emailField.interactable = interactable;
            passwordField.interactable = interactable;
            loginButton.interactable = interactable;
            registerButton.interactable = interactable;
        }
    }
}