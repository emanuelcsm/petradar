export function useToast() {
  function success(_message: string) {}
  function error(_message: string) {}
  function info(_message: string) {}

  return { success, error, info }
}
