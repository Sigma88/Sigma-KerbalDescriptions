using System.Linq;
using UnityEngine;
using KSP.UI;
using KSP.UI.Screens;
using KSP.UI.Screens.SpaceCenter.MissionSummaryDialog;


namespace SigmaKerbalDescriptions
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class Triggers : MonoBehaviour
    {
        static ProtoCrewMember newCrew = null;

        void Start()
        {
            // Crew Assignment Dialog
            Resources.FindObjectsOfTypeAll<CrewAssignmentDialog>().FirstOrDefault().gameObject.AddComponent<AssignmentFix>();

            // Mission Recovery Dialog
            Resources.FindObjectsOfTypeAll<MissionRecoveryDialog>().FirstOrDefault().gameObject.AddComponent<AssignmentFix>();

            // TEST
            CrewWidget[] widgets = Resources.FindObjectsOfTypeAll<CrewWidget>();
            for (int i = 0; i < widgets?.Length; i++)
            {
                if (widgets[i]?.gameObject != null && widgets[i]?.GetComponent<RecoveryFix>() == null)
                    widgets[i].gameObject.AddComponent<RecoveryFix>();
            }

            // Astronaut Complex
            Resources.FindObjectsOfTypeAll<AstronautComplex>().FirstOrDefault().gameObject.AddComponent<AstronautComplexFix>();
            GameEvents.OnCrewmemberHired.Add(HireApplicant);
            GameEvents.OnCrewmemberSacked.Add(FireCrew);
        }

        void HireApplicant(ProtoCrewMember kerbal, int n)
        {
            newCrew = kerbal;
            TimingManager.UpdateAdd(TimingManager.TimingStage.Normal, HireApplicant);
        }
        void HireApplicant()
        {
            Debug.Log("HIRE_UPDATE");
            Description.Update(newCrew);
            Description.UpdateAll(HighLogic.CurrentGame.CrewRoster.Applicants.ToArray());
            Debug.Log("HIRE_UPDATE_FINISH");
            TimingManager.UpdateRemove(TimingManager.TimingStage.Normal, HireApplicant);
        }

        void FireCrew(ProtoCrewMember kerbal, int n)
        {
            TimingManager.UpdateAdd(TimingManager.TimingStage.Normal, FireCrew);
        }
        void FireCrew()
        {
            Debug.Log("FIRE_UPDATE");
            ProtoCrewMember[] applicants = HighLogic.CurrentGame.CrewRoster.Applicants.ToArray();
            Description.UpdateAll(applicants);
            Debug.Log("FIRE_UPDATE_FINISH");
            TimingManager.UpdateRemove(TimingManager.TimingStage.Normal, FireCrew);
        }
    }
}
