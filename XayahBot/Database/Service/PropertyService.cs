﻿using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Utility;

namespace XayahBot.Database.Service
{
    public class PropertyService
    {
        public string GetValue(Property property)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TProperty dbProperty = database.Properties.FirstOrDefault(x => x.Name.Equals(property.Name));
                if (dbProperty != null)
                {
                    return dbProperty.Value;
                }
            }
            return null;
        }

        public Task SetValueAsync(Property property)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TProperty dbProperty = database.Properties.FirstOrDefault(x => x.Name.Equals(property.Name));
                if (dbProperty == null)
                {
                    database.Properties.Add(new TProperty { Name = property.Name, Value = property.Value });
                }
                else
                {
                    dbProperty.Value = property.Value;
                }
                database.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }
    }
}
