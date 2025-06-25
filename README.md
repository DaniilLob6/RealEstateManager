# 🏠 RealEstateManager


## 📌 Описание

**RealEstateManager** — это WPF-приложение на .NET 8 для управления объектами недвижимости, арендаторами и договорами. Программа подключается к базе данных PostgreSQL и поддерживает операции CRUD, а также экспорт и импорт данных в формате JSON.

---

## 🛠️ Функциональные возможности

- ✅ Просмотр, добавление и редактирование **объектов недвижимости**
- 👥 Управление списком **арендаторов**
- 📄 Ведение **договоров аренды**
- 💾 **Экспорт и импорт данных в JSON**
- 🧠 Построено на архитектуре **MVVM**
- 🔄 Использует паттерн **Command (от MVVM Toolkit)**
- 💼 Связь с PostgreSQL через **Entity Framework Core**

---

## 🧱 Архитектура

📦 Проект построен по архитектурному шаблону **MVVM**:

🎯 Используемый паттерн: **Command Pattern**  
🔧 Реализован с помощью `[RelayCommand]` из `CommunityToolkit.Mvvm`

---

## 💻 Как запустить

1. Установи PostgreSQL и создай БД `real_estate_db`
2. Укажи свои данные подключения в `RealEstateContext.cs`:

```csharp
optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=real_estate_db;Username=postgres;Password=yourpassword");


