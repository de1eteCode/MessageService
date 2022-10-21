using System.Diagnostics;

namespace MessageService.Models {

    public class RpsPool {
        private object _lockObject = new object();
        private int _tasksPerSecond;
        private static Stopwatch _sw = Stopwatch.StartNew();

        private RpsPoolItem _currentItem;

        public RpsPool(int tasksPerSecond) {
            if (tasksPerSecond < 1)
                throw new InvalidOperationException();
            _tasksPerSecond = tasksPerSecond;

            var tmpItem = new RpsPoolItem();
            _currentItem = tmpItem;
            while (--tasksPerSecond > 0) {
                tmpItem.Next = new RpsPoolItem();
                tmpItem = tmpItem.Next;
            }
            tmpItem.Next = _currentItem;
        }

        public int TasksPerSecond => _tasksPerSecond;

        public bool CanExecute() {
            lock (_lockObject) {
                var tmpStamp = _sw.ElapsedMilliseconds;
                var res = _currentItem.Next.StartingMilliseconds + 1000 <= tmpStamp;
                if (res) {
                    _currentItem = _currentItem.Next;
                    _currentItem.StartingMilliseconds = tmpStamp;
                }

                return res;
            }
        }
    }
}