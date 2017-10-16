<Query Kind="Program" />

void Main()
{
	string text = "123";
	InPlaceReverser(text.ToCharArray().Dump()).Dump();
}

public char[] InPlaceReverser(char [] text)
{
	if (text == null || text.Length <= 1)
		return text;

	for (var i = 0; i < text.Length / 2; i++)
	{
		var upperIndex = text.Length - 1 - i;
		var temp = text[upperIndex];
		text[upperIndex] = text[i];
		text[i] = temp;
	}
	
	return text;
}