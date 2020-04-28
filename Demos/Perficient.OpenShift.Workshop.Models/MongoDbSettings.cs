namespace Perficient.OpenShift.Workshop.Models
{
    public class MongoDbSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public bool IsValidAuthenticatedConnectionString()
        {
            return !string.IsNullOrEmpty(this.HostName) &&
                this.Port > 0 &&
                !string.IsNullOrEmpty(this.DatabaseName) &&
                !string.IsNullOrEmpty(this.Username) &&
                !string.IsNullOrEmpty(this.Password);
        }

        public string GetConnectionString()
        {
            string connectionString;
            if (this.IsValidAuthenticatedConnectionString())
            {
                connectionString = $"mongodb://{this.Username}:{this.Password}@{this.HostName}:{this.Port}/{this.DatabaseName}";
            }
            else
            {
                connectionString = $"mongodb://{this.HostName}";
            }
            return connectionString;
        }

        // Connection String:  mongodb://mongodb.workshop-demo.svc.cluster.local
        // mongodb://nosql.data
        // mongodb://forecastsuser:forecastpassword@mongodb.workshop-demo.svc.cluster.local:27017/forecastdb
        // database-name
        // database-password
        // database-user
    }
}
