# Checklist de Conformidade Arquitetural — PetRadar

Verificação das regras de isolamento de módulos e comunicação por eventos.
Cada item foi validado lendo os arquivos `.csproj` e o código-fonte diretamente.

---

## 1. Nenhum `ProjectReference` cruzado entre módulos de negócio

**Regra:** `Animals.*`, `Identity.*`, `Media.*` e `Notifications.*` não podem referenciar
diretamente uns aos outros. Referências a `PetRadar.SharedKernel` e
`PetRadar.IntegrationEvents` são permitidas (building blocks).

| Projeto | Referências internas | Cruzamento ilegal? |
|---|---|---|
| `Animals.Domain` | `PetRadar.SharedKernel` | ✅ Não |
| `Animals.Application` | `Animals.Domain`, `PetRadar.IntegrationEvents` | ✅ Não |
| `Animals.Infrastructure` | `Animals.Application` | ✅ Não |
| `Identity.Domain` | `PetRadar.SharedKernel` | ✅ Não |
| `Identity.Application` | `Identity.Domain`, `PetRadar.IntegrationEvents` | ✅ Não |
| `Identity.Infrastructure` | `Identity.Application` | ✅ Não |
| `Media.Domain` | `PetRadar.SharedKernel` | ✅ Não |
| `Media.Application` | `PetRadar.SharedKernel`, `Media.Domain`, `PetRadar.IntegrationEvents` | ✅ Não |
| `Media.Infrastructure` | `Media.Application` | ✅ Não |
| `Notifications.Application` | `PetRadar.SharedKernel`, `PetRadar.IntegrationEvents` | ✅ Não |
| `Notifications.Infrastructure` | `Notifications.Application` | ✅ Não |
| `PetRadar.API` | `Animals.Infrastructure`, `Identity.Infrastructure`, `Media.Infrastructure`, `Notifications.Infrastructure` | ✅ Permitido (raiz de composição) |

**Resultado: CONFORME.** Nenhuma referência ilegal encontrada.

---

## 2. Nenhum contrato síncrono cross-module

**Regra:** um módulo não deve definir interfaces ou tipos concretos que outro módulo
importe diretamente. A única forma de comunicação cross-module é via eventos.

Evidência: os namespaces dos módulos (`Animals.*`, `Identity.*`, `Media.*`,
`Notifications.*`) não aparecem em `using` de nenhum projeto de outro módulo.
A verificação de `ProjectReference` acima é suficiente — sem referência de projeto,
não há como importar tipos de outro módulo.

**Resultado: CONFORME.**

---

## 3. Comunicação cross-module exclusivamente por `IntegrationEvent` via MediatR

**Regra:** toda troca de informação entre módulos deve ocorrer por meio de
`IntegrationEvent` (herdeiro de `INotification`) publicado via `IMediator.Publish()`.

Fluxos identificados no código:

| Módulo origem | IntegrationEvent | Módulo consumidor | Arquivo consumidor |
|---|---|---|---|
| Animals | `AnimalPostedIntegrationEvent` | Notifications | `AnimalPostedIntegrationEventHandler.cs` |
| Animals | `AnimalFoundIntegrationEvent` | Notifications | `AnimalFoundIntegrationEventHandler.cs` |
| Animals | `AnimalPostDeletedIntegrationEvent` | Notifications | `AnimalPostDeletedIntegrationEventHandler.cs` |
| Animals | `AnimalPostDeletedIntegrationEvent` | Media | `AnimalPostDeletedIntegrationEventHandler.cs` |
| Animals | `AnimalTipSentIntegrationEvent` | Notifications | `AnimalTipSentIntegrationEventHandler.cs` |
| Media | `MediaUploadedIntegrationEvent` | Animals | `MediaUploadedIntegrationEventHandler.cs` |
| Identity | `UserRegisteredIntegrationEvent` | *(sem consumidor ativo)* | — |

> `UserRegisteredIntegrationEvent` é publicado pelo bridge em `Identity.Application`
> mas nenhum módulo o consome atualmente. O contrato existe no shared project e está
> disponível para consumo futuro — ausência de consumidor não viola a regra.

**Resultado: CONFORME.**

---

## 4. Bridges `DomainEvent → IntegrationEvent` em todos os módulos que notificam

**Regra:** todo módulo que precisa notificar outros deve ter um
`INotificationHandler<DomainEvent>` na sua camada Application que publica o
`IntegrationEvent` correspondente via `IMediator.Publish()`.

| Módulo | DomainEvent | Bridge (Application) | IntegrationEvent publicado |
|---|---|---|---|
| Animals | `AnimalPostedEvent` | `AnimalPostedIntegrationEventPublisher` | `AnimalPostedIntegrationEvent` |
| Animals | `AnimalFoundEvent` | `AnimalFoundIntegrationEventPublisher` | `AnimalFoundIntegrationEvent` |
| Animals | `AnimalPostDeletedDomainEvent` | `AnimalPostDeletedIntegrationEventPublisher` | `AnimalPostDeletedIntegrationEvent` |
| Animals | `AnimalTipSentDomainEvent` | `AnimalTipSentIntegrationEventPublisher` | `AnimalTipSentIntegrationEvent` |
| Identity | `UserRegisteredEvent` | `UserRegisteredIntegrationEventPublisher` | `UserRegisteredIntegrationEvent` |
| Media | `MediaUploadedDomainEvent` | `MediaUploadedIntegrationEventPublisher` | `MediaUploadedIntegrationEvent` |
| Notifications | *(não produz eventos de domínio)* | — | — |

Todos os bridges seguem o mesmo padrão:

```csharp
public sealed class AnimalPostedIntegrationEventPublisher
    : INotificationHandler<AnimalPostedEvent>
{
    public async Task Handle(AnimalPostedEvent notification, CancellationToken ct)
    {
        var integrationEvent = new AnimalPostedIntegrationEvent(...);
        await _mediator.Publish(integrationEvent, ct);
    }
}
```

**Resultado: CONFORME.** Todos os módulos que possuem domain events que precisam
cruzar fronteira têm o bridge correspondente na camada Application.

---

## 5. Notifications opera exclusivamente sobre contratos de `PetRadar.IntegrationEvents`

**Regra:** `Notifications.Application` não deve referenciar tipos de domínio de outros
módulos. Deve operar apenas sobre os contratos de `PetRadar.IntegrationEvents`.

Evidência — `Notifications.Application.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="PetRadar.SharedKernel.csproj" />
  <ProjectReference Include="PetRadar.IntegrationEvents.csproj" />
</ItemGroup>
```

Sem referência a `Animals.*`, `Identity.*` ou `Media.*`. Os handlers em
`Notifications.Application.Integration.Consumers` importam apenas tipos de
`PetRadar.IntegrationEvents.Animals`.

**Resultado: CONFORME.**

---

## Resumo

| Regra | Status |
|---|---|
| Nenhum `ProjectReference` cruzado entre módulos de negócio | ✅ CONFORME |
| Nenhum contrato síncrono cross-module | ✅ CONFORME |
| Comunicação cross-module exclusivamente por `IntegrationEvent` via MediatR | ✅ CONFORME |
| Bridges `DomainEvent → IntegrationEvent` em todos os módulos que notificam | ✅ CONFORME |
| `Notifications` opera apenas sobre contratos de `PetRadar.IntegrationEvents` | ✅ CONFORME |

**A aplicação está totalmente conforme com as regras de isolamento e eventos.**
Nenhum ajuste é necessário.
