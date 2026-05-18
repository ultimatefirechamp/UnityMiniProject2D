using System;
using UnityEditor.Rendering;
using UnityEngine;

public enum TestState
{
    NONE,
    Suceeded,
    Failed
}
public interface ISyncTest
{
    TestState Status { get; }
    event Action<SyncTestHandle> Completed;
    string GetResult();
}
public struct SyncTestHandle // AsyncOperationHandle
{
    ISyncTest _syncTest;
    public SyncTestHandle(ISyncTest syncTest)
    {
        this._syncTest = syncTest;
    }
    public TestState State { get { return _syncTest.Status; } }
    public event Action<SyncTestHandle> Completed
    {
        add { _syncTest.Completed += value; }
        remove { _syncTest.Completed -= value; }
    }

    public string Result
    {
        get
        {
            return _syncTest.GetResult();
        }
    }
}

public class Dummy : ISyncTest
{
    public TestState Status { get; private set; } = TestState.NONE; //private set으로 설정 가능하게. NONE으로 초기화
    public event Action<SyncTestHandle> Completed; 
    private string _loadedData;
    public string GetResult()
    { return _loadedData; }
    public void FinishWork(string loadedData, SyncTestHandle handler) //무언가 얘를 받아서 하는 작업이 끝나면 FinishWork()호출
    {
        this._loadedData = loadedData;
        if(this._loadedData == null)
        {
            Status = TestState.Failed;
        }
        else
        {
            Status = TestState.Suceeded;
        }
        Completed?.Invoke(handler);
    }
}
public static class test
{
    public static SyncTestHandle TestLoadAssest(string data)
    {
        Dummy dum = new Dummy();
        SyncTestHandle handle = new SyncTestHandle(dum);
        // 뭔지는 모르지만 마법의 try load를 코드. 일단 다음 줄로 흘려보냄. 나중에 dum에 대한 데이터가 막 수정될거임.
        //3초 뒤에 dum.FinishWork(data);
        return handle;
        // 고민했던 것들 있었나요? 어떻게 해결했나요? 이런 경험들을 말씀해주세요
    }
}

public class PracticeSyncTest
{
    void MyPractice()
    {
        SyncTestHandle handle = test.TestLoadAssest("power");
        
        handle.Completed += (op) =>
        {
            if(op.State == TestState.Suceeded)
            {
                Debug.Log("TEST Suceed");
            }
            else
            {
                Debug.Log("TEST Fail");
            }
        };
    }
}