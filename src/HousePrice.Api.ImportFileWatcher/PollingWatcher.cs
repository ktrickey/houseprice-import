namespace HousePrice.Api.ImportFileWatcher
{
    public class PollingWatcher
    {
        private readonly FilePoller _poller;
        public PollingWatcher(FilePoller poller)
        {
            _poller = poller; 
        }

        public void StartPolling()
        {
            //_poller.StartPolling();
        }
    }
}