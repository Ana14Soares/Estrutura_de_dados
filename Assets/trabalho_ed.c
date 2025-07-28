#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>

#define MAX_TAREFA 100
#define TYPE_PILHA 1

#ifdef _WIN32
#define EXPORT __declspec(dllexport)
#else
#define EXPORT
#endif


//------- PILHA -------

typedef struct tagFila{
    int type;
    char *buffer;
    char *first;
    char *last;
    int size;
    int sizeElement;
    int maxElement;
} TFila;

bool Fila_create(TFila *fila, int sizeElement, int max, int type);
void Fila_destroy(TFila *fila);
bool Fila_put(TFila *fila, char *data);
bool Fila_get(TFila *fila,char *data);
bool Fila_isEmpty(TFila *fila);
bool Fila_isFull(TFila *fila);


EXPORT bool Fila_create(TFila *fila, int sizeElement, int max, int type){
    if(fila == NULL || sizeElement == 0 || max == 0) return false;
    fila->buffer = malloc(sizeElement * max);
    if(fila->buffer == NULL) return false;
    fila->size = 0;
    fila->sizeElement = sizeElement;
    fila->maxElement = max;
    fila->first = fila->buffer;
    fila->last = fila->buffer;
    fila->type = type;
    return true;
}

EXPORT void Fila_destroy(TFila *fila){
    free(fila->buffer);
    fila->size = 0;
    fila->sizeElement = 0;
    fila->maxElement = 0;
    fila->first = NULL;
    fila->last = NULL;
}


EXPORT bool Fila_put(TFila *fila, char *data){
    if(data == NULL) return false;
    if(Fila_isFull(fila)) return false;
    
    memcpy(fila->last, data, fila->sizeElement);
    fila->last += fila->sizeElement;
    
    if(fila->type == TYPE_PILHA){
        if(fila->last >= fila->buffer + fila->maxElement * fila->sizeElement){
            fila->last = fila->buffer;
        }
    }
    
    fila->size++;
    return true;
}


EXPORT bool Fila_get(TFila *fila, char *data){
    if(data == NULL) return false;
    
    if(fila->type == TYPE_PILHA){
        if(Fila_isEmpty(fila)) return false;
        fila->last -= fila->sizeElement;
        memcpy(data, fila->last, fila->sizeElement);
        fila->size--;
        return true;
    }else{
        if(Fila_isEmpty(fila)) return false;
        memcpy(data, fila->first, fila->sizeElement);
        fila->first += fila->sizeElement;
        if(fila->first >= fila->buffer + fila->maxElement * fila->sizeElement){
            fila->first = fila->buffer;
        }
        fila->size--;
        return true;
    }
}

EXPORT bool Fila_isEmpty(TFila *fila){
    return fila->size == 0;
}

EXPORT bool Fila_isFull(TFila *fila){
    return fila->size == fila->maxElement;
}

//---------Lista---------

typedef struct tagElementoLista{
    char tarefa[MAX_TAREFA];
    struct tagElementoLista *next;
}TElementoLista;

typedef struct tagLista{
    TElementoLista *first;
    TElementoLista *last;
    int nElementos;
} TLista;

void Lista_cria(TLista *lista);
bool Lista_inserir(TLista *lista, const char *texto);
void Lista_mostrar(TLista *lista, char *buffer);
bool Lista_removerPorNome(TLista *lista, const char *nome);
void Lista_obterTodas(TLista *lista, char *buffer, int bufferSize);

EXPORT void Lista_cria(TLista *lista) {
    lista->first = NULL;
    lista->last = NULL;
    lista->nElementos = 0;
}


EXPORT void Lista_obterTodas(TLista *lista, char *buffer, int bufferSize) {
    buffer[0] = '\0'; // Zera o buffer

    TElementoLista *atual = lista->first;
    while (atual) {
        if ((strlen(buffer) + strlen(atual->tarefa) + 2) >= bufferSize) {
            // Evita estouro
            break;
        }
        strcat(buffer, atual->tarefa);
        strcat(buffer, "\n");
        atual = atual->next;
    }
}


EXPORT bool Lista_inserir(TLista *lista, const char *texto) {
    TElementoLista *novo = malloc(sizeof(TElementoLista));
    if (!novo) return false;
    strcpy(novo->tarefa, texto);
    novo->next = NULL;
    if (lista->nElementos == 0) {
        lista->first = novo;
    } else {
        lista->last->next = novo;
    }
    lista->last = novo;
    lista->nElementos++;
    return true;
}

EXPORT void Lista_mostrar(TLista *lista, char *buffer) {
    TElementoLista *atual = lista->first;
    int i = 1;
    buffer[0] = '\0'; // limpa o buffer
    while (atual != NULL) {
        char temp[128];
        sprintf(temp, "%d. %s\n", i, atual->tarefa);
        strcat(buffer, temp);
        atual = atual->next;
        i++;
    }
}



EXPORT bool Lista_removerPorNome(TLista *lista, const char *nome) {
    if (lista == NULL || lista->nElementos == 0) return false;

    TElementoLista *atual = lista->first;
    TElementoLista *anterior = NULL;

    while (atual != NULL) {
        if (strcmp(atual->tarefa, nome) == 0) {
            // Encontrou o elemento, copia o texto
            // (Se não precisar retornar, pode omitir)
            // strcpy(buffer, atual->tarefa);

            // Ajusta ponteiros
            if (anterior == NULL) {
                // Removendo o primeiro
                lista->first = atual->next;
                if (lista->nElementos == 1)
                    lista->last = NULL;
            } else {
                anterior->next = atual->next;
                if (atual == lista->last)
                    lista->last = anterior;
            }

            free(atual);
            lista->nElementos--;
            return true;
        }

        anterior = atual;
        atual = atual->next;
    }

    // Não encontrou
    return false;
}


int main(){
    
}
