namespace MicroBase.Share.Extensions
{
    public static class PageExtensions
    {
        public static int GetTotalPage(int pageSize, int totalRecord)
        {
            if (totalRecord % pageSize == 0)
            {
                return totalRecord / pageSize;
            }

            return (totalRecord / pageSize) + 1;
        }
    }
}