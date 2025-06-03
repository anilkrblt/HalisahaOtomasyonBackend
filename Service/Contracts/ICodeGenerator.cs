public interface ICodeGenerator
{
    /// <summary>
    /// İstenen uzunlukta ve karakter setinde benzersiz bir kod üretir.
    /// </summary>
    /// <param name="length">Kaç karakterlik kod üretileceği (örneğin 6).</param>
    string Generate(int length);
}
