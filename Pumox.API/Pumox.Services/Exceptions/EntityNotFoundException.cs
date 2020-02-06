using System;
using System.Collections.Generic;
using System.Text;

namespace Pumox.Services.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        private readonly string _message = "Entity {0} not found.";

        public EntityNotFoundException(string entityName)
        {
            EntityName = entityName;
        }

        public override string Message
        {
            get
            {
                return string.Format(_message, EntityName);
            }
        }

        public string EntityName { get; set; }
    }
}
