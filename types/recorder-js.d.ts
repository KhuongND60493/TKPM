declare module 'recorder-js' {
  export default class Recorder {
    constructor(audioContext: AudioContext)

    init(stream: MediaStream): void

    start(): Promise<void>

    stop(): Promise<{ blob: Blob; buffer: AudioBuffer }>

    clearStream(): void

    closeAudioContext(): void
  }
}
