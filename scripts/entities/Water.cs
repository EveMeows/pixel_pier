using Godot;
using System;

public partial class Water : Area2D {
    [Export] private Sprite2D rect;
    [Export] private CollisionShape2D collider;

    public override void _Ready() => OnShapeResized();

    private void OnShapeResized() {
        RectangleShape2D @new = new RectangleShape2D {
            Size = rect.Texture.GetSize() * rect.Scale
        };

        collider.Shape = @new;
        collider.Position = rect.Position;
    }

	private void OnBodyEntered(Node2D body) {
		if (body is Player player) {
			if (player.LastKnownSafePosition is not null) {
				player.Position = player.LastKnownSafePosition.Value;
			}
		}
	}
}
	