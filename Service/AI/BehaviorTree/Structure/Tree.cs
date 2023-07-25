using Game_Realtime.Model;

namespace Game_Realtime.Service.AI.BehaviorTree.Structure
{
    public abstract class Tree
    {
        protected Node root = new Node();

        public Tree()
        {
            Console.WriteLine("Call from Tree()");
            //root = SetUpTree();
        }

        public void Update()
        {
            if (root != null)
            {
                root.Evaluate();
            }
        }

        protected abstract Node SetUpTree();
    }
}
