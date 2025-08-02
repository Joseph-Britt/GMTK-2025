using UnityEngine;

public class TestData : MonoBehaviour {

    [SerializeField] private int data = -1;

    public void SetData(int data) {
        this.data = data;
    }
    
}
