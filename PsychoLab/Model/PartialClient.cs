namespace PsychoLab.Model
{
    public partial class Client
    {
        public string GetData
        {
            get
            {
                return $"{FirstName} {LastName} - {Phone} ";
            }
        }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName} {MiddleName} ";
            }
        }
    }
}
