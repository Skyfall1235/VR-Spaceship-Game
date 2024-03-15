using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class SystemResourceQueue
{
    public int EngineVal
    {
        get => Engines.Count;
    }
    public int InternalSystemsVal
    {
        get => InternalSystems.Count;
    }
    public int WeaponsVal
    {
        get => Weapons.Count;
    }

    Stack<int> totalItems = new(7);

    Stack<int> Engines = new(5);
    Stack<int> InternalSystems = new(5);
    Stack<int> Weapons = new(5);

    public UnityEvent OnUpdateSystemResourceQueue = new();
    public UnityEvent OnFailToAllocate = new();

    /// <summary>
    /// Enumeration representing the different types of system resources.
    /// </summary>
    public enum PipSelection
    {
        Engine,
        internalSystems,
        Weapons
    }

    /// <summary>
    /// Reallocates resources based on the selected PipSelection (engine, internal systems, or weapons).
    /// Attempts to allocate a resource from the total stack first, then from the highest non-selected stack if the total stack is empty.
    /// </summary>
    /// <param name="selection">The PipSelection enum value indicating the desired resource type.</param>
    public void ReallocateResource(PipSelection selection)
    {
        //we need to conver the enum to the stack
        Stack<int> selectedStack = ConvertEnumToStackRef(selection);

        //confirm the stack has capacity before pursuing further logic
        if (selectedStack.Count >= 5) { return; }

        //so the allocator should attempt to follow this cycle
        //1-take from the total items stack
        if (totalItems.Count > 0)
        {
            PopnPush(totalItems, selectedStack);
        }

        //2-if nothing in that stack, take from the biggest stack that isnt itself
        PopnPush(DetermineHighestNonSameStack(selectedStack), selectedStack);
    }

    internal void PopnPush(Stack<int> popStack, Stack<int> pushStack)
    {
        popStack.Pop();
        pushStack.Push(0);//value doesnt matter
    }
    internal Stack<int> ConvertEnumToStackRef(PipSelection selection)
    {
        switch (selection)
        {
            case PipSelection.Engine:
                return Engines;

            case PipSelection.internalSystems:
                return InternalSystems;

            case PipSelection.Weapons:
                return Weapons;

            default: return null;
        }
    }

    internal Stack<int> DetermineHighestNonSameStack(Stack<int> refStack)
    {
        //AI GENERATED (problem stumped me for 2 days)

        // Get the stack counts excluding the reference stack
        int enginesCount = Engines.Count;
        int internalSystemsCount = InternalSystems.Count;
        int weaponsCount = Weapons.Count;

        // Temporarily remove the reference stack's count
        if (refStack == Engines) enginesCount = 0;
        else if (refStack == InternalSystems) internalSystemsCount = 0;
        else if (refStack == Weapons) weaponsCount = 0;

        // Find the stack with the highest count among the remaining stacks
        int highestCount = Math.Max(enginesCount, Math.Max(internalSystemsCount, weaponsCount));

        if (highestCount == enginesCount)
        {
            return Engines;
        }
        else if (highestCount == internalSystemsCount)
        {
            return InternalSystems;
        }
        else
        {
            return Weapons;
        }
    }
}
