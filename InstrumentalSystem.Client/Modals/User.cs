namespace InstrumentalSystem.Client.Modals
{
    public class User
    {
        public int Id { get; private set; }
        public int AccountId { get; private set; }
        public string FullName { get; private set; }
        public string Role { get; private set; }
        public string Organization { get; private set; }
        public string Login { get; private set; }

        public User(int id, int accountId, string fullName, string role, string organization, string login)
        {
            Id = id;
            AccountId = accountId;
            FullName = fullName;
            Role = role;
            Organization = organization;
            Login = login;
        }
        
        public override string ToString()
        {
            return Login;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is User otherUser)
            {
                return Id == otherUser.Id;
            }
            
            return false;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}