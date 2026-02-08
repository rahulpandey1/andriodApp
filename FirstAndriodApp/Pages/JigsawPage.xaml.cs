using FirstAndriodApp.PageModels;

namespace FirstAndriodApp.Pages;

public partial class JigsawPage : ContentPage
{
    public JigsawPage(JigsawPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    JigsawPageModel Model => (JigsawPageModel)BindingContext;

    public void Reset_Clicked(object sender, EventArgs e) => Model.Reset();

    private double startX, startY;

    private async void Piece_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is not Image img || img.BindingContext is not JigsawPiece piece) return;
        if (piece.IsLocked) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                startX = piece.CurrentX;
                startY = piece.CurrentY;
                piece.ZIndex = 100; // Bring to front
                break;
                
            case GestureStatus.Running:
                piece.CurrentX = startX + e.TotalX;
                piece.CurrentY = startY + e.TotalY;
                break;
                
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                // Check Snap - Coordinates are relative to Grid/AbsoluteLayout (TopLeft 0,0)
                // Board TopLeft is at 20,20 (Hardcoded in XAML)
                // TargetX/Y in ViewModel are relative to 0,0 of Board.
                // So we need to subtract Board Offset (20,20) from Piece Position to get Drop Position relative to board.
                
                double boardOffset = 20;
                double dropX = piece.CurrentX - boardOffset;
                double dropY = piece.CurrentY - boardOffset;

                bool snapped = await Model.TrySnap(piece, dropX, dropY);
                if (snapped)
                {
                    piece.CurrentX = piece.TargetX + boardOffset;
                    piece.CurrentY = piece.TargetY + boardOffset;
                }
                else
                {
                    piece.ZIndex = 10;
                }
                break;
        }
    }
}
