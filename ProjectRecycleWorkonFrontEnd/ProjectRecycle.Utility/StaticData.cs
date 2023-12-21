namespace ProjectRecycle.Utility
{
    public static class StaticData
    {
        public enum WasteType
        {
            Plastic,
            Metal,
            Concrete,
            Wood,
            Paper,
            hazardous
        }

        public enum MissionStatus
        {
            Pending,
            Approved,
            Completed,
            Rejected
        }
        public enum CompanyActivity
        {
            indiustrial,
            commercial,
            construction,
            argriculture
        }
        public enum UserRole
        {
            Admin,
            Consultant
        }
    }
}