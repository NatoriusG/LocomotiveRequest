using CommsRadioAPI;

namespace LocomotiveRequest;

// TODO: Comment
internal class RadioLocomotiveRequestEntry : AStateBehaviour
{
    internal RadioLocomotiveRequestEntry() :
        base(new CommsRadioState
        (
            titleText: "LOCO REQUEST",
            contentText: "Request locomotive delivery?",
            buttonBehaviour: DV.ButtonBehaviourType.Regular
        ))
    {

    }

    public override AStateBehaviour OnAction(CommsRadioUtility utility, InputAction action)
    {
        utility.PlaySound(VanillaSoundCommsRadio.Warning);
        return new RadioLocomotiveRequestSelect();
    }
}

internal class RadioLocomotiveRequestSelect : AStateBehaviour
{
    private readonly int selectedIndex;

    internal RadioLocomotiveRequestSelect(int selectedIndex = 0) :
        base(new CommsRadioState
        (
            titleText: "LOCO REQUEST",
            contentText: LocomotiveRequest.AvailableLocomotives[selectedIndex].Name,
            actionText: "CONFIRM",
            buttonBehaviour: DV.ButtonBehaviourType.Override
        ))
    {
        LocomotiveRequest.LogDebug("Constructing new selection screen");
        LocomotiveRequest.LogDebug($"selected index: {selectedIndex}");
        this.selectedIndex = selectedIndex;
    }

    public override AStateBehaviour OnAction(CommsRadioUtility utility, InputAction action)
    {
        var locomotives = LocomotiveRequest.AvailableLocomotives;
        var maxIndex = locomotives.Count - 1;
        bool valid;
        int newIndex;

        switch (action)
        {
            case InputAction.Activate:
                var selectedLocomotive = locomotives[selectedIndex];
                LocomotiveRequest.LogDebug($"Requesting spawn of locomotive: {selectedLocomotive.Name}");
                var result = LocomotiveRequest.TrySpawnRequestedLocomotive(selectedLocomotive, out string message);
                utility.PlaySound(result ? VanillaSoundCommsRadio.Confirm : VanillaSoundCommsRadio.Warning);
                return new RadioLocomotiveRequestConfirmation(message);

            case InputAction.Down:
                valid = selectedIndex < maxIndex;
                newIndex = valid ? selectedIndex + 1 : maxIndex;
                utility.PlaySound(valid ? VanillaSoundCommsRadio.Switch : VanillaSoundCommsRadio.Warning);
                return new RadioLocomotiveRequestSelect(newIndex);

            case InputAction.Up:
                valid = selectedIndex > 0;
                newIndex = valid ? selectedIndex - 1 : 0;
                utility.PlaySound(valid ? VanillaSoundCommsRadio.Switch : VanillaSoundCommsRadio.Warning);
                return new RadioLocomotiveRequestSelect(newIndex);

            default:
                return new RadioLocomotiveRequestEntry();
        }
    }
}

internal class RadioLocomotiveRequestConfirmation : AStateBehaviour
{
    internal RadioLocomotiveRequestConfirmation(string confirmationMessage) :
        base(new CommsRadioState
        (
            titleText: "LOCO REQUEST",
            contentText: confirmationMessage,
            actionText: "DONE",
            buttonBehaviour: DV.ButtonBehaviourType.Ignore
        ))
    { }

    public override AStateBehaviour OnAction(CommsRadioUtility utility, InputAction action)
    {
        utility.PlaySound(VanillaSoundCommsRadio.Confirm);
        return new RadioLocomotiveRequestEntry();
    }
}
