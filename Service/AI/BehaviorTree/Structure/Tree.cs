namespace Game_Realtime.Service.AI.BehaviorTree.Structure
{
    public abstract class Tree
    {
        private Node root = new Node();

        public Tree()
        {
            root = SetUpTree();
        }

        private void Update()
        {
            if (root != null)
            {
                root.Evaluate();
            }
        }

        protected abstract Node SetUpTree();
    }
}
