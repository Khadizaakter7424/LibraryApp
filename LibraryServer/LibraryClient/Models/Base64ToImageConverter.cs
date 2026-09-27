using System.Globalization;

namespace LibraryClient.Models
{
    // Turns the Base64 string in Member.ImageUrl into something an <Image> can show.
    public class Base64ToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter,
                               CultureInfo culture)
            => ToImageSource(value as string);

        public object? ConvertBack(object? value, Type targetType, object? parameter,
                                   CultureInfo culture)
            => throw new NotSupportedException();

        public static ImageSource? ToImageSource(string? base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                return null;

            try
            {
                byte[] bytes = System.Convert.FromBase64String(base64);
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}