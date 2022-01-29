namespace SG
{
    partial class Form1
    {
        private void SelectEvent(SGEvent s)
        {
            selectedEvent.selected = false;
            selectedEvent = s;
            selectedEvent.selected = true;
        }

        private void UnselectEvent(SGEvent s)
        {
            s.selected = false;
        }

        private void UnselectAll()
        {
            UnselectEvent(selectedEvent);
            UnselectJob();
            foreach (SGEvent s in sglist)
            {
                foreach (SGJob j in s.childs)
                {
                    //j.showInCycle = false;
                    j.showBackPath = false;
                    j.showForwardPath = false;
                }

            }
        }

        private void SelectJob(SGJob j)
        {
            if (j != null)
            {
                selectedJob.selected = false;
                selectedJob = j;
                selectedJob.selected = true;
            }
        }

        
        private void UnselectJob()
        {
            if (selectedJob != null)
            {
                selectedJob.selected = false;
            }
        }
        

        private void UnselectJob(SGJob j)
        {
            j.selected = false;
        }
    }
}