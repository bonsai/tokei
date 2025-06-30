import javax.swing.*;
import java.awt.*;
import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;

public class TimeLeftApp {
    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> {
            JFrame frame = new JFrame("Time Left Today");
            frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
            frame.setSize(900, 900);
            frame.setLocationRelativeTo(null);
            frame.setContentPane(new TimeLeftPanel());
            frame.setVisible(true);
        });
    }
}

class TimeLeftPanel extends JPanel {
    public TimeLeftPanel() {
        setBackground(Color.BLACK);
        // 1秒ごとに再描画
        new Timer(1000, e -> repaint()).start();
    }

    @Override
    protected void paintComponent(Graphics g) {
        super.paintComponent(g);
        Graphics2D g2 = (Graphics2D) g;
        g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);

        LocalDateTime now = LocalDateTime.now();
        LocalDateTime endOfDay = now.withHour(23).withMinute(59).withSecond(59).withNano(999_000_000);
        long secondsLeft = ChronoUnit.SECONDS.between(now, endOfDay);
        int hoursLeft = (int) (secondsLeft / 3600);
        int minutesLeft = (int) ((secondsLeft % 3600) / 60);
        int secsLeft = (int) (secondsLeft % 60);

        // Draw hours (200px squares, 4列)
        g2.setColor(new Color(0x4a90e2));
        for (int i = 0; i < hoursLeft; i++) {
            int row = i / 4;
            int col = i % 4;
            int x = col * 200;
            int y = row * 200;
            g2.fillRect(x, y, 200, 200);
        }

        // Draw minutes (60px squares, 15列)
        g2.setColor(new Color(0x50d2c2));
        int minuteStartY = ((hoursLeft + 3) / 4) * 200; // 切り上げ
        for (int i = 0; i < minutesLeft; i++) {
            int row = i / 15;
            int col = i % 15;
            int x = col * 60;
            int y = minuteStartY + (row * 60);
            g2.fillRect(x, y, 60, 60);
        }

        // Draw seconds (10px squares, 90列)
        g2.setColor(new Color(0xe74c3c));
        int secondStartY = minuteStartY + ((minutesLeft + 14) / 15) * 60; // 切り上げ
        for (int i = 0; i < secsLeft; i++) {
            int row = i / 90;
            int col = i % 90;
            int x = col * 10;
            int y = secondStartY + (row * 10);
            g2.fillRect(x, y, 10, 10);
        }
    }
} 