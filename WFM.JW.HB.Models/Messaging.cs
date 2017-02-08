
namespace WFM.JW.HB.Models
{
    interface iMessage
    {
        void copy<F>(Messaging<F> right);
    }


    public class Messaging<T> : iMessage
    {
        public T Value;
        public int retType;
        public string Message;

        
        public void copy<F>( Messaging<F> right)
        {
            this.Message = right.Message;
            this.retType = right.retType;
        }
    }
}