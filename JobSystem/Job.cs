namespace JobSystem
{
    public abstract class Job
    {
        public void Execute()
        {
            Handle();
            OnHandled();
        }

        protected abstract void Handle();

        protected abstract void OnHandled();
    }
}
