namespace Wexflow.Server.Contracts.Workflow
{
    public class WorkflowInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LaunchType { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsApproval { get; set; }

        public bool HasRestParams { get; set; }

        public string Description { get; set; }

        public string Period { get; set; }

        public string CronExpression { get; set; }

        public string WorkiomUsername { get; set; }

        public string WorkiomPassword { get; set; }

        public string WorkiomTenantName { get; set; }

        public Variable[] LocalVariables { get; set; }

    }
}
