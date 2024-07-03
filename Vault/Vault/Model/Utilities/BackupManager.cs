using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Model.Uilities
{
    public sealed class BackupManager
    {
        public static BackupManager _instance;
        public static List<BackupJob> _activeJobs = new List<BackupJob>();
        public event EventHandler UpdateBackupJob;
        private DateTime _lastUpdate = DateTime.Now;

        public static BackupManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new BackupManager();
            }
            return _instance;
        }

        // Add remove job to list
        public void AddActiveJob(BackupJob job)
        {
            lock (_activeJobs)
            {
                if (!_activeJobs.Contains(job))
                {
                    _activeJobs.Add(job);
                }
            }
        }

        public void RemoveActiveJob(BackupJob job)
        {
            lock (_activeJobs)
            {
                if (_activeJobs.Contains(job))
                {
                    _activeJobs.Remove(job);
                }
            }
        }

        public List<BackupJob> GetJobs()
        {
            return _activeJobs;
        }

        public void OnUpdateBackupJob(string updateType="trough")
        {
            if (updateType == "blocked")
            {
                if (DateTime.Now.Subtract(_lastUpdate).TotalMilliseconds < 400)
                {
                    return;
                }
            }
            _lastUpdate = DateTime.Now;

            UpdateBackupJob?.Invoke(this, EventArgs.Empty);

            
        }
        public void Stop(int index)
        {
            lock (_activeJobs[index].realtimeStats)
            {
                _activeJobs[index].realtimeStats.IsStoped = true;
            }
        }

        public void PauseResume(int index)
        { 
            if (index > _activeJobs.Count || index < 0)
            {
                return;
            }
            lock (_activeJobs[index].realtimeStats)
            {
                if (_activeJobs[index].realtimeStats.IsPaused)
                {
                    _activeJobs[index].realtimeStats.IsPaused = false;
                }
                else
                {
                    _activeJobs[index].realtimeStats.IsPaused = true;
                }
            }
            OnUpdateBackupJob();
        }
        public void PauseResume(BackupJob job)
        {
            lock (job.realtimeStats)
            {
                if (job.realtimeStats.IsPaused)
                {
                    job.realtimeStats.IsPaused = false;
                }
                else
                {
                    job.realtimeStats.IsPaused = true;
                }
            }
            OnUpdateBackupJob();
        }


        public void PauseAllActiveJobs()
        {
            foreach (BackupJob job in _activeJobs)
            {
                job.realtimeStats.IsPaused = true;
            }
        }
        public void ResumeAllActiveJobs()
        {

            foreach (BackupJob job in _activeJobs)
            {
                job.realtimeStats.IsPaused = false;
            }
        }

    }

}
