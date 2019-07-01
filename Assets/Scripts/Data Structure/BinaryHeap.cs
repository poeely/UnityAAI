/* Author: Vincent J.S. Versnel
 * */

// An abstract binary heap data structure
public interface IHeapNode
{
    int index { get; set; }
    int value { get; set; }
}

public class BinaryHeap<T> where T : IHeapNode
{
    private T[] nodes;
    private int count;

    private int GetLeftChildIdx(int parentIndex)  { return 2 * parentIndex + 1; }
    private int GetRightChildIdx(int parentIndex) { return 2 * parentIndex + 2; }
    private int GetParentIdx(int childIndex)      { return (childIndex - 1) / 2; }

    public BinaryHeap(int maxSize)
    {
        nodes = new T[maxSize];
    }

    // Adds an node to the heap and sorts it
    public void Insert(T node, int value)
    {
        node.index = count;
        node.value = value;
        nodes[count] = node;

        count++;

        HeapifyUp(node);
    }

    // Removes and returns the top node at the heap
    public T Extract()
    {
        T nodeToExtract = nodes[0];
        
        count--;

        SwapPlaces(0, count);

        HeapifyDown(nodes[0]);

        return nodeToExtract;
    }

    // Sorts heap upwards from a given node
    public void HeapifyUp(T node)
    {
        int index = node.index;
        int parentIndex = GetParentIdx(index);

        while (parentIndex >= 0 && Difference(index, parentIndex) < 0)
        {
            SwapPlaces(index, parentIndex);
            index = parentIndex;
            parentIndex = GetParentIdx(index);
        } 
    }

    // Sorts heap downwards from a given node
    private void HeapifyDown(T node)
    {
        int index = node.index;

        while (index < count)
        {
            int smallestChild = index;

            int left = GetLeftChildIdx(index);
            int right = GetRightChildIdx(index);
            
            if(left < count)
            {
                smallestChild = left;

                if(right < count && Difference(right, smallestChild) < 0)
                {
                    smallestChild = right;
                }
            }

            if (Difference(smallestChild, index) >= 0)
                break;

            SwapPlaces(index, smallestChild);
            index = smallestChild;
        }
    }

    // swaps two items from firstIndex to secondIndex
    private void SwapPlaces(int firstIndex, int secondIndex)
    {
        // swap place
        T nodeToSwap = nodes[firstIndex];
        nodes[firstIndex] = nodes[secondIndex];
        nodes[secondIndex] = nodeToSwap;

        // swap index
        nodes[firstIndex].index = firstIndex;
        nodes[secondIndex].index = secondIndex;
    }

    private bool HasParent(int index)
    {
        return GetParentIdx(index) < count;
    }

    // Compares the difference between two values
    private int Difference(int a, int b)
    {
        return nodes[a].value - nodes[b].value;
    }

    // returns true if the array contains the given item, similar to .Contains from Lists
    public bool Contains(T item)
    {
        return Equals(nodes[item.index], item);
    }

    public int Count { get { return count; } }
}