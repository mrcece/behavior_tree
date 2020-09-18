namespace Bright.BehaviorTree
{
    public abstract class AbstractEventJob
    {
        public AbstractNode Node { get; }

        public int Version { get; }

        public bool Canceled => Node.Version != Version;

        protected AbstractEventJob(AbstractNode node)
        {
            Node = node;
            Version = node.Version;
        }

        public abstract void Execute();
    }
}
