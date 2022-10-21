namespace MessageService.Models {

    internal class RpsPoolItem {
        public RpsPoolItem Next { get; set; }
        public long StartingMilliseconds { get; set; } = -1000;
    }
}