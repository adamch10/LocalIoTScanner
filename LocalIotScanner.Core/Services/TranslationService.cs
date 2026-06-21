using System;
using System.Collections.Generic;

namespace LocalIotScanner.Web.Services
{
    public class TranslationService
    {
        public string CurrentLanguage { get; private set; } = "en"; // Default to English

        // Event that tells the UI to re-render when the language changes
        public event Action? OnLanguageChanged;

        public void SetLanguage(string languageCode)
        {
            if (CurrentLanguage != languageCode)
            {
                CurrentLanguage = languageCode;
                OnLanguageChanged?.Invoke();
            }
        }

        // This Indexer allows us to use a super clean syntax in HTML: @T["Key"]
        public string this[string key]
        {
            get
            {
                if (_translations.TryGetValue(key, out var translations) &&
                    translations.TryGetValue(CurrentLanguage, out var text))
                {
                    return text;
                }
                return key; // Fallback: If translation is missing, just display the key name
            }
        }

        // Add as many translations here as you need!
        private readonly Dictionary<string, Dictionary<string, string>> _translations = new()
        {
            // Sidebar Navigation
            { "Nav_Dashboard", new() { { "en", "Dashboard" }, { "pl", "Panel Główny" } } },
            { "Nav_History", new() { { "en", "Audit History" }, { "pl", "Historia Audytów" } } },
            
            // Dashboard Header
            { "Dash_Title", new() { { "en", "Local Network Security Audit" }, { "pl", "Audyt Bezpieczeństwa Sieci Lokalnej" } } },
            { "Dash_Subtitle", new() { { "en", "One-click vulnerability assessment for your connected devices." }, { "pl", "Jednym kliknięciem oceń podatności swoich podłączonych urządzeń." } } },
            { "Dash_QuickScan", new() { { "en", "Quick Scan" }, { "pl", "Szybki Skan" } } },
            { "Dash_ExtendedScan", new() { { "en", "Extended Scan" }, { "pl", "Rozszerzony Skan" } } },
            { "Dash_StartBtn", new() { { "en", "Start Audit" }, { "pl", "Rozpocznij Audyt" } } },
            { "Dash_Advanced", new() { { "en", "Advanced Settings" }, { "pl", "Ustawienia Zaawansowane" } } },
            { "Dash_QuickDesc", new() { { "en", "Scans the most common IoT ports (HTTP, SSH, Telnet, FTP) for rapid assessment." }, { "pl", "Skanuje najpopularniejsze porty IoT (HTTP, SSH, Telnet, FTP) dla szybkiej oceny." } } },
            { "Dash_ExtendedDesc", new() { { "en", "Scans a comprehensive list of ports including databases, media streams (RTSP), and remote desktops." }, { "pl", "Skanuje obszerną listę portów, w tym bazy danych, strumienie wideo (RTSP) i pulpity zdalne." } } },
            { "Dash_Scanning", new() { { "en", "Scanning Network..." }, { "pl", "Skanowanie Sieci..." } } },
            { "Dash_SubnetLabel", new() { { "en", "Target Subnet (Auto-Detected)" }, { "pl", "Podsieć Docelowa (Wykryto Automatycznie)" } } },
            { "Dash_PortsLabel", new() { { "en", "Target Ports (Comma Separated)" }, { "pl", "Porty Docelowe (Oddzielone Przecinkami)" } } },
            { "Dash_Status", new() { { "en", "Status" }, { "pl", "Status" } } },
            { "Dash_DevicesFound", new() { { "en", "Devices Found" }, { "pl", "Znalezione Urządzenia" } } },
            { "Dash_VulnsFound", new() { { "en", "Vulnerabilities" }, { "pl", "Podatności" } } },
            { "Dash_AuditComplete", new() { { "en", "Audit Complete" }, { "pl", "Audyt Zakończony" } } },
            { "Dash_OverallRisk", new() { { "en", "Overall Risk" }, { "pl", "Ogólne Ryzyko" } } },
            { "Dash_PortsOpen", new() { { "en", "Ports Open" }, { "pl", "Otwarte Porty" } } },
            { "Dash_Issues", new() { { "en", "Issues" }, { "pl", "Zagrożenia" } } },
            { "Dash_Secure", new() { { "en", "Secure" }, { "pl", "Bezpieczne" } } },
            { "Dash_NoPortsMsg", new() { { "en", "No open ports detected. This device appears secure." }, { "pl", "Nie wykryto otwartych portów. Urządzenie wydaje się bezpieczne." } } },
            { "Dash_TablePort", new() { { "en", "Port / Service" }, { "pl", "Port / Usługa" } } },
            { "Dash_TableVulns", new() { { "en", "Vulnerabilities & Mitigations" }, { "pl", "Podatności i Mitygacja" } } },
            { "Dash_NoVulnsMsg", new() { { "en", "No known vulnerabilities detected for this service." }, { "pl", "Nie wykryto znanych podatności dla tej usługi." } } },
           

            { "Critical", new() { { "en", "Critical" }, { "pl", "Krytyczne" } } },
            { "High", new() { { "en", "High" }, { "pl", "Wysokie" } } },
            { "Medium", new() { { "en", "Medium" }, { "pl", "Średnie" } } },
            { "Low", new() { { "en", "Low" }, { "pl", "Niskie" } } },
            { "None", new() { { "en", "None" }, { "pl", "Brak" } } },
            

            { "Unencrypted Web Interface", new() { { "en", "Unencrypted Web Interface" }, { "pl", "Nieszyfrowany Interfejs Webowy" } } },
            { "An HTTP service is exposed. If this is an administrative panel, login credentials could be intercepted.", new() { { "en", "An HTTP service is exposed. If this is an administrative panel, login credentials could be intercepted." }, { "pl", "Wykryto otwartą usługę HTTP. Jeśli to panel administracyjny, dane logowania mogą zostać przechwycone." } } },
            { "Ensure the device uses HTTPS, or isolate it within a strict VLAN.", new() { { "en", "Ensure the device uses HTTPS, or isolate it within a strict VLAN." }, { "pl", "Upewnij się, że urządzenie używa HTTPS lub odizoluj je w ścisłej sieci VLAN." } } },
            
            { "Hist_Title", new() { { "en", "Audit History" }, { "pl", "Historia Audytów" } } },
            { "Hist_Subtitle", new() { { "en", "Review previous network security scans and findings." }, { "pl", "Przeglądaj poprzednie skany bezpieczeństwa sieci i wykryte zagrożenia." } } },
            { "Hist_Loading", new() { { "en", "Loading..." }, { "pl", "Wczytywanie..." } } },
            { "Hist_NoData", new() { { "en", "No audit history found. Run a scan from the dashboard to see results here!" }, { "pl", "Brak historii audytów. Uruchom skan na panelu głównym, aby zobaczyć wyniki!" } } },
            { "Hist_Hosts", new() { { "en", "Hosts" }, { "pl", "Hosty" } } },
            { "Hist_NoDevices", new() { { "en", "No devices were found during this scan." }, { "pl", "Podczas tego skanu nie znaleziono żadnych urządzeń." } } },
            { "Hist_ColIP", new() { { "en", "IP Address" }, { "pl", "Adres IP" } } },
            { "Hist_ColType", new() { { "en", "Device Type" }, { "pl", "Typ Urządzenia" } } },
            { "Hist_ColPorts", new() { { "en", "Open Ports & Services" }, { "pl", "Otwarte Porty i Usługi" } } },
            { "Hist_ColVulns", new() { { "en", "Detected Vulnerabilities" }, { "pl", "Wykryte Podatności" } } },
            
            { "Status_Init", new() { { "en", "Initializing scan session..." }, { "pl", "Inicjowanie sesji skanowania..." } } },
            { "Status_ICMP", new() { { "en", "Running ICMP discovery..." }, { "pl", "Wykonywanie mapowania ICMP..." } } },
            { "Status_PortScan", new() { { "en", "Starting port scans..." }, { "pl", "Rozpoczynanie skanowania portów..." } } },
            { "Status_Scanning", new() { { "en", "Scanning devices..." }, { "pl", "Skanowanie urządzeń..." } } },
            { "Status_Risk", new() { { "en", "Calculating overall network risk..." }, { "pl", "Obliczanie ogólnego ryzyka sieci..." } } },
            { "Status_Saving", new() { { "en", "Saving audit results to local database..." }, { "pl", "Zapisywanie wyników do bazy danych..." } } },
            { "Status_Done", new() { { "en", "Audit completed successfully." }, { "pl", "Audyt zakończony pomyślnie." } } },

            // --- Podatności: Telnet ---
            { "Cleartext Protocol Detected (Telnet)", new() { { "en", "Cleartext Protocol Detected (Telnet)" }, { "pl", "Wykryto protokół tekstowy (Telnet)" } } },
            { "Telnet transmits all data, including credentials, in plain text. This is highly susceptible to packet sniffing.", new() { { "en", "Telnet transmits all data, including credentials, in plain text. This is highly susceptible to packet sniffing." }, { "pl", "Telnet przesyła wszystkie dane, w tym dane logowania, jawnym tekstem. Jest to wysoce podatne na podsłuchiwanie pakietów (sniffing)." } } },
            { "Disable Telnet and use a secure protocol like SSH (Port 22).", new() { { "en", "Disable Telnet and use a secure protocol like SSH (Port 22)." }, { "pl", "Wyłącz Telnet i użyj bezpiecznego protokołu, takiego jak SSH (Port 22)." } } },
            
            // --- Podatności: FTP ---
            { "Insecure File Transfer (FTP)", new() { { "en", "Insecure File Transfer (FTP)" }, { "pl", "Niezabezpieczony transfer plików (FTP)" } } },
            { "FTP sends authentication data in cleartext.", new() { { "en", "FTP sends authentication data in cleartext." }, { "pl", "FTP przesyła dane uwierzytelniające jawnym tekstem." } } },
            { "Upgrade the service to SFTP or FTPS.", new() { { "en", "Upgrade the service to SFTP or FTPS." }, { "pl", "Zaktualizuj usługę do SFTP lub FTPS." } } },
            
            // --- Podatności: RTSP (Kamery IP) ---
            { "Exposed Media Stream (RTSP)", new() { { "en", "Exposed Media Stream (RTSP)" }, { "pl", "Otwarty strumień multimedialny (RTSP)" } } },
            { "RTSP is frequently used by IP cameras. If unauthenticated, video feeds can be intercepted or viewed by unauthorized users.", new() { { "en", "RTSP is frequently used by IP cameras. If unauthenticated, video feeds can be intercepted or viewed by unauthorized users." }, { "pl", "Protokół RTSP jest często używany przez kamery IP. Bez uwierzytelnienia, strumienie wideo mogą być przechwytywane lub oglądane przez nieuprawnionych użytkowników." } } },
            { "Ensure RTSP requires strong authentication or place the camera on an isolated network.", new() { { "en", "Ensure RTSP requires strong authentication or place the camera on an isolated network." }, { "pl", "Upewnij się, że RTSP wymaga silnego uwierzytelnienia, lub umieść kamerę w odizolowanej sieci VLAN." } } },
            
            // --- Podatności: MQTT (Smart Home) ---
            { "Unencrypted MQTT Broker", new() { { "en", "Unencrypted MQTT Broker" }, { "pl", "Nieszyfrowany broker MQTT" } } },
            { "MQTT is used for IoT telemetry. Without TLS (Port 8883), sensor data and control commands are transmitted in plaintext.", new() { { "en", "MQTT is used for IoT telemetry. Without TLS (Port 8883), sensor data and control commands are transmitted in plaintext." }, { "pl", "Protokół MQTT służy do telemetrii IoT. Bez TLS (port 8883), dane z czujników i polecenia sterujące są przesyłane otwartym tekstem." } } },
            { "Configure the broker to use MQTTS (TLS) and require client authentication.", new() { { "en", "Configure the broker to use MQTTS (TLS) and require client authentication." }, { "pl", "Skonfiguruj brokera do używania MQTTS (TLS) i wymagaj uwierzytelniania klientów." } } },
            
            // --- Podatności: Domyślne Hasła HTTP ---
            { "Default Credentials Configured", new() { { "en", "Default Credentials Configured" }, { "pl", "Skonfigurowano domyślne dane logowania" } } },
            { "Change the default password immediately.", new() { { "en", "Change the default password immediately." }, { "pl", "Natychmiast zmień domyślne hasło." } } },
            
            // Permutacje dla dynamicznego opisu (zależnie od tego, jakie hasło zadziałało)
            { "The device allowed access using factory default credentials (admin:admin). This allows immediate administrative takeover.", new() { { "en", "The device allowed access using factory default credentials (admin:admin). This allows immediate administrative takeover." }, { "pl", "Urządzenie zezwoliło na dostęp przy użyciu fabrycznych danych logowania (admin:admin). Umożliwia to natychmiastowe przejęcie kontroli administracyjnej." } } },
            { "The device allowed access using factory default credentials (admin:password). This allows immediate administrative takeover.", new() { { "en", "The device allowed access using factory default credentials (admin:password). This allows immediate administrative takeover." }, { "pl", "Urządzenie zezwoliło na dostęp przy użyciu fabrycznych danych logowania (admin:password). Umożliwia to natychmiastowe przejęcie kontroli administracyjnej." } } },
            { "The device allowed access using factory default credentials (admin:1234). This allows immediate administrative takeover.", new() { { "en", "The device allowed access using factory default credentials (admin:1234). This allows immediate administrative takeover." }, { "pl", "Urządzenie zezwoliło na dostęp przy użyciu fabrycznych danych logowania (admin:1234). Umożliwia to natychmiastowe przejęcie kontroli administracyjnej." } } },
            { "The device allowed access using factory default credentials (root:root). This allows immediate administrative takeover.", new() { { "en", "The device allowed access using factory default credentials (root:root). This allows immediate administrative takeover." }, { "pl", "Urządzenie zezwoliło na dostęp przy użyciu fabrycznych danych logowania (root:root). Umożliwia to natychmiastowe przejęcie kontroli administracyjnej." } } },
            { "The device allowed access using factory default credentials (root:admin). This allows immediate administrative takeover.", new() { { "en", "The device allowed access using factory default credentials (root:admin). This allows immediate administrative takeover." }, { "pl", "Urządzenie zezwoliło na dostęp przy użyciu fabrycznych danych logowania (root:admin). Umożliwia to natychmiastowe przejęcie kontroli administracyjnej." } } }
        };
    }
}