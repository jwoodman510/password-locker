namespace Ksu.Model
{
    public class FlattenedServerLogin
    {
        public int ServerLoginId { get; set; }
        public int ServerId { get; set; }
        public int DepartmentId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DepartmentName { get; set; }
        public string ServerName { get; set; }
    }
}
