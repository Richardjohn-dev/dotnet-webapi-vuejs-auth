namespace RoleBasedIdentityAuthentication.API.Data
{
    public class AdminSeed
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UsernameUpdate { get; set; } = string.Empty;
        public string PasswordUpdate { get; set; } = string.Empty;

        private bool _update = false;
        public bool Update
        {
            get => _update;
            set
            {
                if (!string.IsNullOrEmpty(PasswordUpdate) && PasswordUpdate != Password ||
                    !string.IsNullOrEmpty(UsernameUpdate) && UsernameUpdate != Username)
                {
                    _update = true;
                }
            }
        }
    }
}
