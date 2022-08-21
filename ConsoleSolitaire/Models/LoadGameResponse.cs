namespace ConsoleSolitaire.Models
{
    internal class LoadGameResponse : SaveStateResponse
    {
        public bool DoesExist
        {
            get
            {
                return base.FileInfo.Exists;
            }
        }

        public bool Success
        {
            get
            {
                return this.DoesExist;
            }
        }
    }
}
