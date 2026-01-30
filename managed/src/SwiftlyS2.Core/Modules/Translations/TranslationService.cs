using SwiftlyS2.Core.Natives;
using SwiftlyS2.Core.Services;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.Translation;

namespace SwiftlyS2.Core.Translations;

internal class TranslationService : ITranslationService
{
    private CoreContext _Context { get; init; }
    private TranslationResource _TranslationResource { get; set; } = new();

    public TranslationService( CoreContext context )
    {
        _Context = context;

        CreateTranslationResource();
    }

    public void CreateTranslationResource()
    {
        var translationDir = Path.Combine(_Context.BaseDirectory, "resources", "translations");

        if (!Directory.Exists(translationDir))
        {
            return;
        }

        if (!File.Exists(Path.Combine(translationDir, NativeServerHelpers.GetServerLanguage() + ".jsonc")))
        {
            return;
        }

        _TranslationResource = TranslationFactory.Create(translationDir)!;
    }

    public Language GetServerLanguage()
    {
        return new Language(NativeServerHelpers.GetServerLanguage());
    }

    public Localizer GetLocalizer()
    {
        return _TranslationResource.Resources.Count == 0
        ? new Localizer([], [])
        : TranslationFactory.CreateLocalizer(_TranslationResource, GetServerLanguage());
    }

    public ILocalizer GetPlayerLocalizer( IPlayer player )
    {
        if (_TranslationResource.Resources.Count == 0)
        {
            return new Localizer([], []);
        }

        var language = NativeServerHelpers.UsePlayerLanguage() ? player.PlayerLanguage : GetServerLanguage();
        return TranslationFactory.CreateLocalizer(_TranslationResource, language);
    }
}