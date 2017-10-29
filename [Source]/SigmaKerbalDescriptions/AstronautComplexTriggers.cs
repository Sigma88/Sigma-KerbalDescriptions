using System.Linq;
using UnityEngine;
using KSP.UI.Screens;


namespace SigmaKerbalDescriptions
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class AstronautComplexTriggers : MonoBehaviour
    {
        static ProtoCrewMember newCrew = null;

        void Start()
        {
            Resources.FindObjectsOfTypeAll<AstronautComplex>().FirstOrDefault().gameObject.AddComponent<DescriptionsFixer>();
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
            Description.UpdateAll(HighLogic.CurrentGame.CrewRoster.Applicants.ToArray(), fixListItem: false);
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
