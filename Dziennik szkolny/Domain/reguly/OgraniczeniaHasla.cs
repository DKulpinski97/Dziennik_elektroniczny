namespace Dziennik_szkolny.Domain.reguly
{
    public static class OgraniczeniaHasla
    {
        public const int MinimalnaDlugosc = 8;

        public static readonly string KomunikatBledu =
            $"Hasło musi zawierać co najmniej {MinimalnaDlugosc} znaków, w tym: jedną cyfrę, jedną małą literę, jedną wielką literę oraz jeden znak specjalny.";
        public static readonly string RegulyHasla =
        $"Co najmniej {MinimalnaDlugosc} znaków: mała i wielka litera, cyfra i znak specjalny.";
        public static readonly string PodpowiedzEdycji =
        $"Hasło jest opcjonalne.";
    }
}
