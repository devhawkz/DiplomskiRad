using Client.Pages.Shared;

namespace Client.Services.DialogServices;

public class MessageDialogService
{
    //def se promenljiva messageDialog koja je tipa MessageDialog i pomocu nje se pristupa komponenti MessageDialog
    public MessageDialog? messageDialog;

    public bool PrikaziBusyDugme { get; set; }
    public bool PrikaziDugmeSacuvaj { get; set; } = true;
    
    public Action? Action { get; set; }

    public async void SetMessageDialog()
    {
        // ceka se na prikazivanje pop-up-a
        await messageDialog!.PrikaziPoruku();

        // skriva se prikazivane busy dugmeta
        PrikaziBusyDugme = false;
        
        //prikazuje se sacuvaj dugme
        PrikaziDugmeSacuvaj = true;

        //poziva se delegat
        Action?.Invoke();
    }
}
