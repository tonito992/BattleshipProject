namespace tonigames.battleship.MVC.Base
{
    public class Controller<M> where M: BaseModel {

        protected M Model;

        public virtual void Setup (M model) {
            this.Model = model;
        }
    }
}