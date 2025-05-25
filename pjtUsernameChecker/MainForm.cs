using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace pjtUsernameChecker
{

    /*
     * I am using JetBrains Rider instead of Visual Studio and it tends to be temperamental with the designer
     * and I prefer text based editing anyhow, so I've typed everything for the layout and styling instead of using the drag and drop GUI
     * for the application design. I have made two versions of this application, one strictly utilizing WinForms,
     * and another using the Eto.Forms I discussed with you during open office hours. Eto uses StackLayout and WinForms uses TableLayoutPanel,
     * both of which I had to research before I could finish this assignment.
     */
    public partial class MainForm : Form
    {
        #region variables
        private readonly TextBox _txbUsernameInput;
        private readonly ListBox _lsbTriedNames;
        private readonly Label _lblCheckReturn;
        private readonly List<string> _lstNameHistory;
        private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_]+$", RegexOptions.Compiled);
        private const int MaxHistoryItems = 100;
        private readonly object _historyLock = new object();
        #endregion
        
        public MainForm()
        {
            InitializeComponent();

            MinimumSize = new Size(400, 500);
            Text = "Username Checker";
        
            _lstNameHistory = new List<string>();
            
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                RowCount = 4,
                ColumnCount = 1
            };

            
            Label instructionsLabel = new Label
            {
                Text = "Enter a username (3-24 characters, alphanumerics and underscores only):",
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };
            mainLayout.Controls.Add(instructionsLabel, 0, 0);

            
            FlowLayoutPanel inputPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            _txbUsernameInput = new TextBox
            {
                PlaceholderText = "Enter a username",
                Width = 200,
                MaxLength = 24
            };

            Button checkButton = new Button
            {
                Text = "Check Name",
                Height = 30,
                Width = 100,
                Margin = new Padding(5, 0, 0, 0)
            };

            var lblCharacterCount = new Label
            {
                AutoSize = true,
                Text = "0/24 characters",
                ForeColor = Color.Gray,
                Margin = new Padding(5, 0, 0, 0)
            };
            inputPanel.Controls.Add(_txbUsernameInput);
            inputPanel.Controls.Add(checkButton);
            inputPanel.Controls.Add(lblCharacterCount);
            mainLayout.Controls.Add(inputPanel, 0, 1);

            
            _lblCheckReturn = new Label
            {
                Text = "Enter a username to check",
                ForeColor = Color.Gray,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };
            mainLayout.Controls.Add(_lblCheckReturn, 0, 2);

            
            GroupBox historyGroup = new GroupBox
            {
                Text = "Previously Checked Names",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            _lsbTriedNames = new ListBox
            {
                Dock = DockStyle.Fill
            };

            historyGroup.Controls.Add(_lsbTriedNames);
            mainLayout.Controls.Add(historyGroup, 0, 3);
            
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Controls.Add(mainLayout);

            
            checkButton.Click += CheckUsername;
            
            _txbUsernameInput.TextChanged += (s, e) =>
            {
                int currentLength = _txbUsernameInput.Text.Length;
                lblCharacterCount.Text = $"{currentLength}/24 characters";
                lblCharacterCount.ForeColor = currentLength == 24 ? Color.Red : Color.Gray;
                if (currentLength > 0) //This resets the feedback label, but only when the user starts typing
                {
                    _lblCheckReturn.Text = "Enter a username to check";
                    _lblCheckReturn.ForeColor = Color.Gray;
                }
            };

            _txbUsernameInput.KeyPress += (s, e) =>
            {
                //Lets Enter key act as Check button click
                if (e.KeyChar == (char)Keys.Enter)
                {
                    CheckUsername(s, e);
                    e.Handled = true;
                }
            };
        }

        [AllowNull] public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        //Seal gets Rider to stop complaining about a possibly null value for window size even though it's impossible for a window size to be null...
        public sealed override Size MinimumSize
        {
            get => base.MinimumSize;
            set => base.MinimumSize = value;
        }

        private void CheckUsername(object? sender, EventArgs e)
        {
            string username = _txbUsernameInput.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                _lblCheckReturn.Text = "Please enter a username.";
                _txbUsernameInput.Text = string.Empty;
                return;
            }

            var isValid = IsValidUsername(username);
            _lblCheckReturn.Text = isValid ? "Username is valid!" : "Username is invalid.";
            _lblCheckReturn.ForeColor = isValid ? Color.Green : Color.Red;

            lock (_historyLock)
            {
                if (!_lstNameHistory.Contains(username))
                {
                    _lstNameHistory.Add(username);
                    UpdateListBox(username, isValid);
                }
            }
            
            _txbUsernameInput.Text = string.Empty;
        }

        private void UpdateListBox(string username, bool isValid)
        {
            _lsbTriedNames.Items.Add($"{username} - {(isValid ? "Valid" : "Invalid")}");
            if (_lstNameHistory.Count > MaxHistoryItems)
            {
                _lstNameHistory.RemoveAt(0);
                _lsbTriedNames.Items.RemoveAt(0);
            }
        }

        private bool IsValidUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;
            
            if (username.Length < 3 || username.Length > 24)
                return false;
            
            return UsernameRegex.IsMatch(username);
        }
        
    }
}