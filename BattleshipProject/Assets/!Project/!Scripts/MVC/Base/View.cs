using UnityEngine;

namespace tonigames.battleship.MVC.Base
{
    public abstract class View<TM,TC> : MonoBehaviour where TM : BaseModel where TC : Controller<TM>, new()
    {
        public TM Model;
        protected TC Controller;

        public virtual void Awake () {
            this.Controller = new TC();
            this.Controller.Setup(this.Model);
        }
    }
}