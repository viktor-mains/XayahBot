﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XayahBot.Database.Model;
using XayahBot.Error;

namespace XayahBot.Database.DAO
{
    public class IncidentsDAO
    {
        public TIncident GetIncident()
        {
            return null;
        }

        public async Task AddAsync(TIncident entry)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncident match = database.Incidents.FirstOrDefault(x => x.IncidentId.Equals(entry.IncidentId) && x.UpdateId.Equals(entry.UpdateId));
                if (match == null)
                {
                    database.Incidents.Add(entry);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new AlreadyExistingException();
                }
            }
        }

        public async Task UpdateAsync(long incidentId, string updateId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                TIncident match = database.Incidents.FirstOrDefault(x => x.IncidentId.Equals(incidentId));
                if (match != null)
                {
                    match.UpdateId = updateId;
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new NotExistingException();
                }
            }
        }

        public async Task RemoveAsync(long incidentId)
        {
            using (GeneralContext database = new GeneralContext())
            {
                List<TIncident> matches = database.Incidents.Where(x => x.IncidentId.Equals(incidentId)).ToList();
                if (matches.Count > 0)
                {
                    database.RemoveRange(matches);
                    if (await database.SaveChangesAsync() <= 0)
                    {
                        throw new NotSavedException();
                    }
                }
                else
                {
                    throw new NotExistingException();
                }
            }
        }
    }
}