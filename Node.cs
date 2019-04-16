public class Node
{
	public int diff;
	public int typeOfEnd;
	public Node next;

	public Node(int diff, int typeOfEnd, Node next)
	{
		this.diff = diff;
		this.typeOfEnd = typeOfEnd;
		this.next = next;
	}
}