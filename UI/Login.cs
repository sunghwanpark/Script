using SHProject.Network;

namespace SHProject.Ingame.UI
{
    public class Login : MonoBehaviourBase
    {
        [SetField(typeof(UIInput), "id")]
        private UIInput _input;

        [SetField(typeof(UIButton), "loginButton")]
        private UIButton _btnLogin;

        protected override void Awake()
        {
            base.Awake();

            EventDelegate.Add(_btnLogin.onClick, () =>
            {
                IngameNetwork.Instance.Login(_input.value);
                gameObject.SetActive(false);
            });
        }
    }
}