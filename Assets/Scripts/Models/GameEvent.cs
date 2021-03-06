﻿using UnityEngine;
using System.Collections.Generic;
using MoonSharp;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

[MoonSharpUserData]
public class GameEvent 
{

    public string Name { get; protected set; }
    public bool Repeat { get; protected set; }

    protected List<string> preconditions;
    protected List<string> executionActions;

    private bool executed;
    private float timer;

    public GameEvent(string name, bool repeat){
        Name = name;
        Repeat = repeat;
        preconditions = new List<string>();
        executionActions = new List<string>();
        timer = 0;
    }

    public void Update(float deltaTime){
        int conditionsMet = 0;
        foreach(string precondition in preconditions){
            // Call lua precondition it should return 1 if met otherwise 0
            conditionsMet += (int)(GameEventActions.CallFunction(precondition, this, deltaTime).Number);
        }

        if(conditionsMet >= preconditions.Count && executed == false){
            Execute();
        }
    }

    public void AddTimer(float time)
    {
        timer += time;
    }

    public float GetTimer()
    {
        return timer;
    }

    public void ResetTimer()
    {
        timer = 0;
    }

    public void Execute()
    {
        if (executionActions != null)
        {
            // Execute Lua code like in Furniture ( FurnitureActions ) 
            GameEventActions.CallFunctionsWithEvent(executionActions.ToArray(), this);
        }

        if(!Repeat)
            executed = true;
    }

    public void RegisterPrecondition(string luaFunctionName)
    {
        preconditions.Add(luaFunctionName);
    }

    public void RegisterPreconditions(string[] luaFunctionNames)
    {

        preconditions.AddRange(luaFunctionNames);
    }

    public void RegisterExecutionAction(string luaFunctionName)
    {
        executionActions.Add(luaFunctionName);
    }

    public void RegisterExecutionActions(string[] luaFunctionNames)
    {

        executionActions.AddRange(luaFunctionNames);
    }
}
